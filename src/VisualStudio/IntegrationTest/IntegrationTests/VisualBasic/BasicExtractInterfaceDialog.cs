﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.VisualStudio.IntegrationTest.Utilities;
using Microsoft.VisualStudio.IntegrationTest.Utilities.OutOfProcess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Test.Utilities;

using ProjectUtils = Microsoft.VisualStudio.IntegrationTest.Utilities.Common.ProjectUtils;

namespace Roslyn.VisualStudio.IntegrationTests.VisualBasic
{
    [TestClass]
    public class BasicExtractInterfaceDialog : AbstractEditorTest
    {
        protected override string LanguageName => LanguageNames.VisualBasic;

        private ExtractInterfaceDialog_OutOfProc ExtractInterfaceDialog => VisualStudioInstance.ExtractInterfaceDialog;

        public BasicExtractInterfaceDialog( )
            : base( nameof(BasicExtractInterfaceDialog))
        {
        }

        [TestMethod, TestCategory(Traits.Features.CodeActionsExtractInterface)]
        public void CoreScenario()
        {
            SetUpEditor(@"Class C$$
    Public Sub M()
    End Sub
End Class");

            VisualStudioInstance.Editor.InvokeCodeActionList();
            VisualStudioInstance.Editor.Verify.CodeAction("Extract Interface...",
                applyFix: true,
                blockUntilComplete: false);

            ExtractInterfaceDialog.VerifyOpen();
            ExtractInterfaceDialog.ClickOK();
            ExtractInterfaceDialog.VerifyClosed();

            var project = new ProjectUtils.Project(ProjectName);
            VisualStudioInstance.SolutionExplorer.OpenFile(project, "Class1.vb");

            VisualStudioInstance.Editor.Verify.TextContains(@"Class C
    Implements IC
    Public Sub M() Implements IC.M
    End Sub
End Class");

            VisualStudioInstance.SolutionExplorer.OpenFile(project, "IC.vb");

            VisualStudioInstance.Editor.Verify.TextContains(@"Interface IC
    Sub M()
End Interface");
        }

        [TestMethod, TestCategory(Traits.Features.CodeActionsExtractInterface)]
        public void CheckFileName()
        {
            SetUpEditor(@"Class C2$$
    Public Sub M()
    End Sub
End Class");

            VisualStudioInstance.Editor.InvokeCodeActionList();
            VisualStudioInstance.Editor.Verify.CodeAction("Extract Interface...",
                applyFix: true,
                blockUntilComplete: false);

            ExtractInterfaceDialog.VerifyOpen();

            var fileName = ExtractInterfaceDialog.GetTargetFileName();

            Assert.AreEqual(expected: "IC2.vb", actual: fileName);

            ExtractInterfaceDialog.ClickCancel();
        }
    }
}
