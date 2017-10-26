using System.Diagnostics;

namespace Sorlov.PowerShell.Dto
{
    public class ApplicationVersion
    {
        private FileVersionInfo productVersion;
        private string appName;
        private bool isInstalled;

        internal ApplicationVersion(FileVersionInfo productVersion, string appName)
        {
            this.productVersion = productVersion;
            this.appName = appName;
            if (productVersion == null) isInstalled = false; else isInstalled = true;
        }

        public string ProductName { get { return appName; } }
        public string VersionName
        {
            get
            {
                if (!isInstalled) return string.Empty;

                switch (productVersion.FileMajorPart)
                {
                    case 11:
                        switch (productVersion.FileBuildPart)
                        {
                            case 5614:
                            case 5612:
                            case 5516:
                            case 5531:
                            case 5510:
                            case 5529:
                            case 5525:
                            case 5604:
                            case 3216:
                                return "2003 RTM";
                            case 6355:
                            case 6356:
                            case 6357:
                            case 6361:
                            case 6360:
                            case 6353:
                            case 6707:
                            case 6255:
                            case 6359:
                            case 4301:
                                return "2003 SP1";
                            case 7969:
                                return "2003 SP2";
                            case 8173:
                                return "2003 SP3";
                            default:
                                return string.Format("2003 Build {0}", productVersion.FileBuildPart);
                        }
                    case 12:
                        switch (productVersion.FileBuildPart)
                        {
                            case 4518:
                                return "2007 RTM";
                            case 6213:
                            case 6211:
                            case 6214:
                                return "2007 SP1";
                            case 6425:
                            case 6423:
                            case 6421:
                            case 6413:
                            case 6415:
                                return "2007 SP2";
                            case 6607:
                            case 6606:
                            case 6611:
                            case 6600:
                            case 6612:
                                return "2007 SP3";
                            default:
                                return string.Format("2007 Build {0}", productVersion.FileBuildPart);
                        }
                    case 14:
                        switch (productVersion.FileBuildPart)
                        {
                            case 4760:
                            case 4750:
                            case 4756:
                            case 4763:
                            case 4754:
                            case 4751:
                            case 4762:
                                return "2010 RTM";
                            case 6023:
                            case 6024:
                            case 6009:
                            case 6022:
                            case 6025:
                            case 6026:
                                return "2010 SP1";
                            default:
                                return string.Format("2010 Build {0}", productVersion.FileBuildPart);
                        }
                    case 15:
                        switch (productVersion.FileBuildPart)
                        {
                            case 2703:
                                return "2013 MS2";
                            case 3612:
                                return "2013 PRE";
                            case 4454:
                            case 4481:
                                return "2013 RTM";
                            default:
                                return string.Format("2013 Build {0}", productVersion.FileBuildPart);
                        }
                    default:
                        return string.Format("Version {0} Build {1}", productVersion.FileMajorPart, productVersion.FileBuildPart);
                }
            }
        }
        public string VersionString { get { return (isInstalled ? productVersion.FileVersion : string.Empty); } }
        public bool IsPreRelease { get { return (isInstalled) ? productVersion.IsPreRelease : false; } }
        public bool IsInstalled { get { return isInstalled; } }

    }

}
