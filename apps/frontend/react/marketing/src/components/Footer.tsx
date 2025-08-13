import { Button } from "@/components/ui/button";
import { Facebook, Github, MessageCircle } from "lucide-react";

const Footer = () => {
  const currentYear = new Date().getFullYear();
  return (
    <footer className="bg-card border-t border-border py-16">
      <div className="container mx-auto px-6">
        <div className="grid lg:grid-cols-4 gap-12">
          <div className="lg:col-span-2">
            <div className="text-3xl font-bold bg-gradient-primary bg-clip-text text-transparent mb-6">
              Flyingdarts
            </div>
            <p className="text-muted-foreground text-lg mb-6 max-w-md">
              The premier destination for competitive digital darts. Join our community and
              experience the future of darts gaming.
            </p>

            <div className="space-y-4">
              <div className="text-sm font-medium">Join our community on Discord</div>
              <div className="flex gap-2 max-w-sm">
                <Button asChild className="bg-primary hover:bg-primary/90 shrink-0">
                  <a href="https://discord.gg/SyFzsEbfsk" target="_blank" rel="noreferrer" className="inline-flex items-center gap-2">
                    <MessageCircle />
                    <span>Join Discord</span>
                  </a>
                </Button>
              </div>
            </div>
          </div>

          <div>
            <h3 className="font-semibold text-lg mb-6">Game</h3>
            <div className="space-y-3">
              <a href="https://game.flyingdarts.net" className="block text-muted-foreground hover:text-foreground transition-colors">
                X01
              </a>
            </div>
          </div>

          <div>
            <h3 className="font-semibold text-lg mb-6">Community</h3>
            <div className="space-y-3">
              <a href="https://discord.gg/SyFzsEbfsk" target="_blank" rel="noreferrer" className="block text-muted-foreground hover:text-foreground transition-colors">
                Discord
              </a>
              <a href="https://github.com/flyingdarts/flyingdarts" target="_blank" rel="noreferrer" className="block text-muted-foreground hover:text-foreground transition-colors">
                GitHub
              </a>
              <a href="/community" className="block text-muted-foreground hover:text-foreground transition-colors">
                Support
              </a>
              <a href="/community" className="block text-muted-foreground hover:text-foreground transition-colors">
                Contact
              </a>
            </div>
          </div>
        </div>

          <div className="border-t border-border mt-12 pt-8 flex flex-col sm:flex-row justify-between items-center">
          <div className="text-muted-foreground text-sm mb-4 sm:mb-0">
            © {currentYear} Flyingdarts. All rights reserved.
          </div>

            <div className="flex space-x-4">
              <Button asChild variant="ghost" size="sm" className="text-muted-foreground hover:text-primary">
                <a href="https://www.facebook.com/IYLTDSU" target="_blank" rel="noreferrer" aria-label="Facebook">
                  <Facebook className="w-5 h-5" />
                </a>
              </Button>
              <Button asChild variant="ghost" size="sm" className="text-muted-foreground hover:text-primary">
                <a href="https://github.com/flyingdarts/flyingdarts" target="_blank" rel="noreferrer" aria-label="GitHub">
                  <Github className="w-5 h-5" />
                </a>
              </Button>
            </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
