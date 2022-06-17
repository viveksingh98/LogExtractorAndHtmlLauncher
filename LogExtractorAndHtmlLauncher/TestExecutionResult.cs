/* ***************************************************************
* AzureDevOps Monitoring Application
* © 2022, CCH Incorporated.  All rights reserved.
* Author: Vivek Singh
*****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogsExtractor
{
    class TestExecutionResult
    {
        public int NewAdvisorAccountCreation { get; set; }
        public int AccountActivation { get; set; }
        public int OrganizationCreationForAdvisor { get; set; }
        public int AddingWidgetForDashboard { get; set; }
    }
}
