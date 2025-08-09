import { Button } from "@/components/ui/button";

const Navigation = () => {
  return (
    <nav className="fixed top-0 left-0 right-0 z-50 bg-background/80 backdrop-blur-lg border-b border-border">
      <div className="container mx-auto px-6 py-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-8">
            <div className="text-2xl font-bold bg-gradient-primary bg-clip-text text-transparent">
              Flyingdarts
            </div>
            <div className="hidden md:flex space-x-6">
              <a href="#features" className="text-muted-foreground hover:text-foreground transition-colors">
                Features
              </a>
              <a href="#competitions" className="text-muted-foreground hover:text-foreground transition-colors">
                Competitions
              </a>
              <a href="#community" className="text-muted-foreground hover:text-foreground transition-colors">
                Community
              </a>
            </div>
          </div>
          
          <div className="flex items-center space-x-4">
            <Button variant="ghost" className="hidden sm:flex">
              Sign In
            </Button>
            <Button variant="default" className="bg-gradient-primary hover:opacity-90 transition-opacity">
              Get Started
            </Button>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navigation;