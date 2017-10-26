using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Sorlov.PowerShell.HTTPHandler
{
    public class PSPProcessor :  IHttpHandler
    {
        private Runspace runspace;

        public bool IsReusable
        {
            get { return false; }
        }

        private void Initialize(HttpContext context)
        {
            InitialSessionState sessionState = InitialSessionState.CreateDefault();
            sessionState.Variables.Add(new SessionStateVariableEntry("Request",context.Request,"The IIS request enviroment"));
            sessionState.Variables.Add(new SessionStateVariableEntry("Response",context.Response,"The IIS response enviroment"));
            sessionState.Variables.Add(new SessionStateVariableEntry("Session",context.Session,"The IIS session enviroment"));
            sessionState.Variables.Add(new SessionStateVariableEntry("Application",context.Application,"The IIS application enviroment"));
            sessionState.Variables.Add(new SessionStateVariableEntry("Server",context.Server,"The IIS server enviroment"));
            sessionState.Variables.Add(new SessionStateVariableEntry("Error",context.Error,"The IIS error enviroment"));
            sessionState.Variables.Add(new SessionStateVariableEntry("User",context.User,"The IIS user enviroment"));

            runspace = RunspaceFactory.CreateRunspace(sessionState);
            runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
            runspace.Open();
        }

        private void ExecuteScript(HttpContext context)
        {
            string scriptData = File.ReadAllText(context.Request.PhysicalPath);

            bool parsing = true;
            int currentPos = 0;
            int tagComp = 0;

            List<string> script = new List<string>();
            while(parsing)
            {
                // Calculate start and end position of the tag
                int startingPos = scriptData.IndexOf("<%", currentPos, scriptData.Length - currentPos);
                if (startingPos == -1)
                {
                    script.Add(string.Format("$Response.Write(\"{0}\")", scriptData.Substring(currentPos + 2, scriptData.Length - (currentPos + 2))).Replace(Environment.NewLine, string.Empty).Trim());
                    break;
                };

                // Add before 
                if (currentPos == 0) tagComp = 0; else tagComp = 2;
                script.Add(string.Format("$Response.Write(\"{0}\")", scriptData.Substring(currentPos + tagComp, startingPos - (currentPos + tagComp))).Replace(Environment.NewLine, string.Empty).Trim());


                int endingPos = scriptData.IndexOf("%>", startingPos, scriptData.Length - startingPos);
                if (endingPos == -1)
                {
                    script.Add(string.Format("$Response.Write(\"{0}\")", scriptData.Substring(currentPos, scriptData.Length - currentPos)).Replace(Environment.NewLine, string.Empty).Trim());
                    break;
                }
                
                string[] multiCommand = scriptData.Substring(startingPos+2, (endingPos) - (startingPos+2)).Split(Environment.NewLine.ToCharArray());
                foreach(string cmd in multiCommand) if (!cmd.Trim().Equals(string.Empty)) script.Add(cmd.Trim());

                currentPos = endingPos;
            }

            Collection<PSParseError> errors;
            System.Management.Automation.PSParser.Tokenize(script.ToArray(), out errors);

            if (errors.Count > 0)
            {
                string errorData = string.Empty;
                errorData = "Syntax error: " + errors[0].Message + "<br/><br/><pre>";
                if ((errors[0].Token.StartLine - 2) > -1) errorData += string.Format("{0}<br/>", script[errors[0].Token.StartLine - 2]);
                errorData += string.Format("<font color='red'>{0}</font><br/>", script[errors[0].Token.StartLine - 1]);
                if ((errors[0].Token.StartLine) < script.Count) errorData += string.Format("{0}<br/>", script[errors[0].Token.StartLine]);
                errorData += "</pre>";

                SetInternalResponse(context, 500, "INTERNAL ERROR", "PowerShell Processor Parse Error", errorData);
                return;
            }

            string finalScript=string.Empty;
            foreach (string line in script)
                finalScript += line+Environment.NewLine;

            RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
            scriptInvoker.Invoke(finalScript);
        }

        public void ProcessRequest(HttpContext context)
        {
            
            if (File.Exists(context.Request.PhysicalPath))
            {
                try
                {
                    Initialize(context);
                    ExecuteScript(context);
                }
                catch(Exception ex)
                {
                    
                    SetInternalResponse(context,500, "INTERNAL ERROR","PowerShell Processor Parse Error", string.Format("An error occured during processing of your request: {0}<br/><br/>{1}", ex.Message.Replace(Environment.NewLine, "<br/>"), ex.StackTrace.Replace(Environment.NewLine, "<br/>")));
                }
            }
            else
            {
                SetInternalResponse(context,404,"NOT FOUND","The requested resource could not be found","The requested file could not be found");
            }
        }

        private void SetInternalResponse(HttpContext context, int statusCode, string status, string extendedStatus, string body)
        {
            context.Response.Status = string.Format("{0} {1}",statusCode,status);
            context.Response.Write(string.Format(errorHTML, status, body, context.Request.Path.ToString(),Assembly.GetExecutingAssembly().GetName().Version.ToString(),statusCode.ToString(),extendedStatus));
            context.Response.StatusCode = statusCode;
        }


        private string errorHTML = @"<html>
    <head>
        <title>{4} {0}</title>
        <style>
         body {{font-family:Verdana;font-weight:normal;font-size: .7em;color:black;}} 
         p {{font-family:Verdana;font-weight:normal;color:black;margin-top: -5px}}
         b {{font-family:Verdana;font-weight:bold;color:black;margin-top: -5px}}
         H1 {{ font-family:Verdana;font-weight:normal;font-size:18pt;color:red }}
         H2 {{ font-family:Verdana;font-weight:normal;font-size:14pt;color:maroon }}
         pre {{font-family:Lucida Console;font-size: .9em}}
         .marker {{font-weight: bold; color: black;text-decoration: none;}}
         .version {{color: gray;}}
         .error {{margin-bottom: 10px;}}
         .expandable {{ text-decoration:underline; font-weight:bold; color:navy; cursor:hand; }}
        </style>
    </head>

    <body bgcolor='white'>

            <span><H1>{0}.<hr width=100% size=1 color=silver></H1>

            <h2> <i>{5}</i> </h2></span>

            <font face='Arial, Helvetica, Geneva, SunSans-Regular, sans-serif'>

            <b> Description: </b>{1}
            <br><br>

            <b> Requested URL: </b>{2}<br><br>

            <hr width=100% size=1 color=silver>

            <b>Version Information:</b>&nbsp;Sorlov.PowerShell.HTTPHandler.PSProcessor: {3}

            </font>

    </body>
</html>";
    }
}
