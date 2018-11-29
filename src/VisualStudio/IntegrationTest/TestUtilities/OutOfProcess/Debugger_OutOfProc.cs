﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.IntegrationTest.Utilities.InProcess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.IntegrationTest.Utilities.OutOfProcess
{
    /// <summary>
    /// Provides a means of interacting with the Visual Studio debugger by remoting calls into Visual Studio.
    /// </summary>
    public partial class Debugger_OutOfProc : OutOfProcComponent
    {
        private readonly Debugger_InProc _debuggerInProc;
        private readonly VisualStudioInstance _instance;

        public Debugger_OutOfProc(VisualStudioInstance visualStudioInstance) : base(visualStudioInstance)
        {
            _instance = visualStudioInstance;
            _debuggerInProc = new Debugger_InProc(visualStudioInstance.VisualStudioHost);
        }

        public void SetBreakPoint(string fileName, int lineNumber, int columnIndex)  => 
            _debuggerInProc.SetBreakPoint(fileName, lineNumber, columnIndex);

        public void SetBreakPoint(string fileName, string text, int charsOffset = 0)
        {
            _instance.Editor.SelectTextInCurrentDocument(text);
            int lineNumber = _instance.Editor.GetLine();
            int columnIndex = _instance.Editor.GetColumn();

            SetBreakPoint(fileName, lineNumber, columnIndex + charsOffset);
        }

        public void Go(bool waitForBreakMode) => _debuggerInProc.Go(waitForBreakMode);

        public void StepOver(bool waitForBreakOrEnd) => _debuggerInProc.StepOver(waitForBreakOrEnd);

        public void Stop(bool waitForDesignMode) => _debuggerInProc.Stop(waitForDesignMode);

        public void SetNextStatement() => _debuggerInProc.SetNextStatement();

        public void ExecuteStatement(string statement) => _debuggerInProc.ExecuteStatement(statement);

        public void CheckExpression(string expressionText, string expectedType, string expectedValue)
        {
            var entry = _debuggerInProc.GetExpression(expressionText);
            Assert.AreEqual(expectedType, entry.Type);
            Assert.AreEqual(expectedValue, entry.Value);
        }
    }
}
