class AmplifyConstruct : Construct
{
    private Repository Repository { get; }
    private Branch Main { get; }
    private Branch Staging { get; }
    private Domain Domain { get; }
    private App AmplifyApp { get; }
    public AmplifyConstruct(Construct scope, string id) : base(scope, id)
    {
        // Define the CodeCommit repository
        Repository = new Repository(this, "Flyingdarts-Angular-Repository", new RepositoryProps
        {
            RepositoryName = "Flyingdarts.Angular"
        });

        // Define the Amplify application
        AmplifyApp = new App(this, "Flyingdarts-Angular-Amplify", new AppProps
        {
            SourceCodeProvider = new CodeCommitSourceCodeProvider(new CodeCommitSourceCodeProviderProps
            {
                Repository = Repository
            }),
            AutoBranchCreation = new AutoBranchCreation
            {
                Patterns = new[] { "feature/*"}
            },
            AutoBranchDeletion = true
        });

        // Define main branch
        Main = AmplifyApp.AddBranch("main");
        
        // Define staging branch
        Staging = AmplifyApp.AddBranch("staging");

        // Define a domain
        Domain = AmplifyApp.AddDomain("flyingdarts.net", new DomainOptions
        {
            EnableAutoSubdomain = true,
            AutoSubdomainCreationPatterns = new[] { "*" }
        });

        // Map flyingdarts.net to main branch
        Domain.MapRoot(Main);

        // Map the www subdomain to the main branch
        Domain.MapSubDomain(Main, "www");

        // Map subdomains to development
        Domain.MapSubDomain(Staging);
    }
}

