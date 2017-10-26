using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Threading;

namespace Sorlov.SelfHostedPS.Application
{
    public class Host : PSHost
    {
        private PSHostUserInterface _psHostUserInterface = new HostUserInterface();

        public override void SetShouldExit(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void NotifyBeginApplication()
        {
        }

        public override void NotifyEndApplication()
        {
        }

        public override string Name
        {
            get { return "Sorlov.SelfHostedPS.Application"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0); }
        }

        public override Guid InstanceId
        {
            get { return new Guid("C462AB42-84B6-4143-9589-95FA11EEEEB2"); }
        }

        public override PSHostUserInterface UI
        {
            get { return _psHostUserInterface; }
        }

        public override CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

    }

}