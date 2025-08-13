import loginClient from "@/authressClient";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { ArrowRight, Play } from "lucide-react";
import { useEffect, useState } from "react";

const CTASection = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);

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

  const primaryCtaLabel = isLoggedIn ? "Start Playing Now" : "Join for free";
  return (
    <section className="py-24 relative overflow-hidden">
      <div className="container mx-auto px-6">
        <Card className="relative bg-gradient-hero border-primary/30 overflow-hidden">
          <div className="absolute inset-0 bg-black/20" />

          <div className="relative z-10 p-12 lg:p-20 text-center">
            <h2 className="text-4xl lg:text-6xl font-bold text-white mb-6">
              Ready to Throw Your
              <span className="block text-accent"> Best Game?</span>
            </h2>

            <p className="text-xl text-white/80 mb-10 max-w-2xl mx-auto">
              Join thousands of players who have already discovered the thrill of competitive digital darts.
              Your next perfect game is just one throw away.
            </p>

            <div className="flex flex-col sm:flex-row gap-6 justify-center items-center">
              {isLoggedIn ? (
                <Button
                  asChild
                  size="lg"
                  className="bg-white text-primary hover:bg-white/90 text-lg px-8 py-6 group"
                >
                  <a href="https://game.flyingdarts.net">
                    {primaryCtaLabel}
                    <ArrowRight className="ml-2 w-5 h-5 group-hover:translate-x-1 transition-transform" />
                  </a>
                </Button>
              ) : (
                <Button
                  size="lg"
                  className="bg-white text-primary hover:bg-white/90 text-lg px-8 py-6 group"
                  onClick={() => loginClient.authenticate({ redirectUrl: window.location.href })}
                >
                  {primaryCtaLabel}
                  <ArrowRight className="ml-2 w-5 h-5 group-hover:translate-x-1 transition-transform" />
                </Button>
              )}

              <Button
                size="lg"
                variant="outline"
                disabled
                aria-disabled
                title="Coming soon"
                className="border-white text-white opacity-60 cursor-not-allowed text-lg px-8 py-6 group"
              >
                <Play className="mr-2 w-5 h-5" />
                Watch Gameplay
              </Button>
            </div>

            <div className="mt-12 text-white/60 text-sm">
              No download required • Play instantly in your browser • Free to start
            </div>
          </div>

          {/* Background pattern */}
          <div className="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full -translate-y-32 translate-x-32" />
          <div className="absolute bottom-0 left-0 w-48 h-48 bg-white/5 rounded-full translate-y-24 -translate-x-24" />
        </Card>
      </div>
    </section>
  );
};

export default CTASection;
