﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.VisualStudio.IntegrationTest.Utilities;
using Microsoft.VisualStudio.IntegrationTest.Utilities.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ProjectUtils = Microsoft.VisualStudio.IntegrationTest.Utilities.Common.ProjectUtils;

namespace Roslyn.VisualStudio.IntegrationTests.CSharp
{
    [TestClass]
    public class CSharpBuild : AbstractIntegrationTest
    {
        public CSharpBuild( )
            : base()
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync().ConfigureAwait(true);
            VisualStudioInstance.SolutionExplorer.CreateSolution(nameof(CSharpBuild));
            VisualStudioInstance.SolutionExplorer.AddProject(new ProjectUtils.Project("TestProj"), WellKnownProjectTemplates.ConsoleApplication, LanguageNames.CSharp);
        }

        [TestMethod, TestCategory(Traits.Features.Build)]
        public void BuildProject()
        {
            var editorText = @"using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(""Hello, World!"");
    }
}";

            VisualStudioInstance.Editor.SetText(editorText);

            // TODO: Validate build works as expected
        }

        [TestMethod, Ignore("https://github.com/dotnet/roslyn/issues/18204"), TestCategory(Traits.Features.Build)]
        public void BuildWithCommandLine()
        {
            VisualStudioInstance.SolutionExplorer.SaveAll();

            var pathToDevenv = Path.Combine(VisualStudioInstance.InstallationPath, @"Common7\IDE\devenv.exe");
            var pathToSolution = VisualStudioInstance.SolutionExplorer.SolutionFileFullPath;
            var logFileName = pathToSolution + ".log";

            File.Delete(logFileName);

            var commandLine = $"\"{pathToSolution}\" /Rebuild Debug /Out \"{logFileName}\" {VisualStudioInstanceFactory.VsLaunchArgs}";

            var process = Process.Start(pathToDevenv, commandLine);
            process.WaitForExit();

            ExtendedAssert.Contains("Rebuild All: 1 succeeded, 0 failed, 0 skipped", File.ReadAllText(logFileName));

            Assert.AreEqual(0, process.ExitCode);
        }
    }
}
