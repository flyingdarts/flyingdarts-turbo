import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Facebook, Twitter, Instagram, Youtube } from "lucide-react";

const Footer = () => {
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
              <div className="text-sm font-medium">Stay updated with our newsletter</div>
              <div className="flex gap-2 max-w-sm">
                <Input 
                  placeholder="Enter your email" 
                  className="bg-background border-border"
                />
                <Button className="bg-primary hover:bg-primary/90 shrink-0">
                  Subscribe
                </Button>
              </div>
            </div>
          </div>
          
          <div>
            <h3 className="font-semibold text-lg mb-6">Game</h3>
            <div className="space-y-3">
              <a href="#" className="block text-muted-foreground hover:text-foreground transition-colors">
                Play Now
              </a>
              <a href="#" className="block text-muted-foreground hover:text-foreground transition-colors">
                Tournaments
              </a>
              <a href="#" className="block text-muted-foreground hover:text-foreground transition-colors">
                Leaderboards
              </a>
              <a href="#" className="block text-muted-foreground hover:text-foreground transition-colors">
                Game Rules
              </a>
            </div>
          </div>
          
          <div>
            <h3 className="font-semibold text-lg mb-6">Community</h3>
            <div className="space-y-3">
              <a href="#" className="block text-muted-foreground hover:text-foreground transition-colors">
                Discord
              </a>
              <a href="#" className="block text-muted-foreground hover:text-foreground transition-colors">
                Forums
              </a>
              <a href="#" className="block text-muted-foreground hover:text-foreground transition-colors">
                Support
              </a>
              <a href="#" className="block text-muted-foreground hover:text-foreground transition-colors">
                Contact
              </a>
            </div>
          </div>
        </div>
        
        <div className="border-t border-border mt-12 pt-8 flex flex-col sm:flex-row justify-between items-center">
          <div className="text-muted-foreground text-sm mb-4 sm:mb-0">
            © 2024 Flyingdarts. All rights reserved.
          </div>
          
          <div className="flex space-x-4">
            <Button variant="ghost" size="sm" className="text-muted-foreground hover:text-primary">
              <Facebook className="w-5 h-5" />
            </Button>
            <Button variant="ghost" size="sm" className="text-muted-foreground hover:text-primary">
              <Twitter className="w-5 h-5" />
            </Button>
            <Button variant="ghost" size="sm" className="text-muted-foreground hover:text-primary">
              <Instagram className="w-5 h-5" />
            </Button>
            <Button variant="ghost" size="sm" className="text-muted-foreground hover:text-primary">
              <Youtube className="w-5 h-5" />
            </Button>
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;