using TechTalk.SpecFlow;

namespace Patterns.Specifications.Steps.Ninject.Modules
{
    [Binding]
    public class RuntimeModuleSteps
    {
	    private readonly NinjectContext _context;

	    public RuntimeModuleSteps(NinjectContext context)
		{
			_context = context;
		}

	    [Given(@"I have registered the runtime ninject module")]
        public void GivenIHaveRegisteredTheRuntimeNinjectModule()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
