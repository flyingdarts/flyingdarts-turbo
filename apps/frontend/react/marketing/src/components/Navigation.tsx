import loginClient from "@/authressClient";
import { Button } from "@/components/ui/button";
import { LogOut } from "lucide-react";
import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";

const Navigation = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    let mounted = true;
    (async () => {
      const hasSession = await loginClient.userSessionExists();
      if (mounted) setIsLoggedIn(!!hasSession);
    })();
    return () => {
      mounted = false;
    };
  }, []);

  return (
    <nav className="fixed top-0 left-0 right-0 z-50 bg-background/80 backdrop-blur-lg border-b border-border">
      <div className="container mx-auto px-6 py-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-8">
            <Link to="/" className="text-2xl font-bold bg-gradient-primary bg-clip-text text-transparent">
              Flyingdarts
            </Link>
            <div className="hidden md:flex space-x-6">
              <Link to="/features" className="text-muted-foreground hover:text-foreground transition-colors">Features</Link>
              <Link to="/community" className="text-muted-foreground hover:text-foreground transition-colors">Community</Link>
            </div>
          </div>

          <div className="flex items-center space-x-4">
            {isLoggedIn ? (
              <>
                <Button asChild variant="default" className="bg-gradient-primary hover:opacity-90 transition-opacity">
                  <a href="https://game.flyingdarts.net">Start Gaming</a>
                </Button>
                <Button
                  variant="ghost"
                  size="sm"
                  className="text-muted-foreground"
                  aria-label="Logout"
                  title="Logout"
                  onClick={async () => {
                    await loginClient.logout(window.location.origin);
                  }}
                >
                  <LogOut className="h-4 w-4" />
                </Button>
              </>
            ) : (
              <>
                <Button
                  variant="ghost"
                  className="hidden sm:flex"
                  onClick={() => loginClient.authenticate({ redirectUrl: window.location.href })}
                >
                  Sign In
                </Button>
                <Button
                  variant="default"
                  className="bg-gradient-primary hover:opacity-90 transition-opacity"
                  onClick={() => loginClient.authenticate({ redirectUrl: window.location.href })}
                >
                  Sign Up Now
                </Button>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navigation;
