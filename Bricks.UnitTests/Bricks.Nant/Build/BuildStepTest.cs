using NUnit.Framework;
using System.Reflection;

namespace Bricks.NAnt.Build
{
    [TestFixture]
    public class BuildStepTest
    {
        private string[] callBackArguments;

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            BuildStepTargetForTest.Perform += Callback;
        }

        [SetUp]
        public void SetUp()
        {
            callBackArguments = null;
        }

        [Test]
        public void When_method_doesnt_exist()
        {
            Assert.Throws<BricksBuildException>(() => new BuildStep(typeof(BuildStepTargetForTest), "Doesntexist"));
        }

        [Test]
        public void CapitalizeAction()
        {
            BuildStep buildStep = new BuildStep(typeof(BuildStepTargetForTest), "argumentLessAction");
            buildStep.Execute();
            Assert.AreNotEqual(null, callBackArguments);
            Assert.AreEqual(0, callBackArguments.Length);
        }

        [Test]
        public void ExecuteStepWithoutArguments()
        {
            BuildStep buildStep = new BuildStep(typeof(BuildStepTargetForTest), "ArgumentLessAction");
            buildStep.Execute();
            Assert.AreNotEqual(null, callBackArguments);
            Assert.AreEqual(0, callBackArguments.Length);
        }

        [Test]
        public void Execute_step_with_arguments_without_specifying_the_arguments()
        {
            Assert.Throws<BricksBuildException>(() => new BuildStep(typeof(BuildStepTargetForTest), "ActionWithArgument"));
            Assert.IsNull(callBackArguments);
        }

        [Test]
        public void Execute_step_with_arguments()
        {
            string arg = "Arg";
            BuildStep buildStep = new BuildStep(typeof(BuildStepTargetForTest), "ActionWithArgument", arg);
            buildStep.Execute();
            Assert.AreNotEqual(null, callBackArguments);
            Assert.AreEqual(1, callBackArguments.Length);
            Assert.AreEqual(arg, callBackArguments[0]);
        }

        [Test]
        public void Execute_step_with_variable_arguments()
        {
            BuildStep buildStep = new BuildStep(typeof(BuildStepTargetForTest), "TakesVariableArgument", "arg1", "arg2");
            Assert.Throws<TargetParameterCountException>(() => buildStep.Execute());
        }

        void Callback(string[] arguments)
        {
            callBackArguments = arguments;
        }
    }

    internal class BuildStepTargetForTest
    {
        internal static event ActionPerformed Perform = delegate {};
        internal delegate void ActionPerformed(string[] arguments);

        public static void ArgumentLessAction()
        {
            Perform(new string[0]);
        }

        public static void ActionWithArgument(string arg)
        {
            Perform(new string[]{arg});
        }

        public static void TakesVariableArgument(params string[] args){}
    }
}