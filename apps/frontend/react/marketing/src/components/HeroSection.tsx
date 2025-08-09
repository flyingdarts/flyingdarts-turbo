import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import GeometricShapes from "./GeometricShapes";
import heroImage from "@/assets/hero-darts.jpg";

const HeroSection = () => {
  return (
    <section className="relative min-h-screen flex items-center justify-center overflow-hidden pt-20">
      <GeometricShapes />
      
      <div className="container mx-auto px-6 relative z-10">
        <div className="grid lg:grid-cols-2 gap-12 items-center">
          <div className="text-center lg:text-left animate-slide-up">
            <h1 className="text-5xl lg:text-7xl font-bold mb-6 leading-tight">
              Master the Art of
              <span className="block bg-gradient-primary bg-clip-text text-transparent">
                Digital Darts
              </span>
            </h1>
            
            <p className="text-xl text-muted-foreground mb-8 max-w-2xl">
              Join the ultimate online darts community. Challenge friends, compete in tournaments, 
              and perfect your aim in the most immersive digital darts experience.
            </p>
            
            <div className="flex flex-col sm:flex-row gap-4 justify-center lg:justify-start">
              <Button 
                size="lg" 
                className="bg-gradient-primary hover:opacity-90 transition-opacity text-lg px-8 py-6 animate-pulse-glow"
              >
                Start Playing Now
              </Button>
              <Button 
                size="lg" 
                variant="outline" 
                className="border-primary text-primary hover:bg-primary hover:text-primary-foreground text-lg px-8 py-6"
              >
                Watch Demo
              </Button>
            </div>
            
            <div className="flex items-center justify-center lg:justify-start space-x-8 mt-12">
              <div className="text-center">
                <div className="text-3xl font-bold text-primary">10K+</div>
                <div className="text-sm text-muted-foreground">Active Players</div>
              </div>
              <div className="text-center">
                <div className="text-3xl font-bold text-accent">500+</div>
                <div className="text-sm text-muted-foreground">Daily Matches</div>
              </div>
              <div className="text-center">
                <div className="text-3xl font-bold text-primary">24/7</div>
                <div className="text-sm text-muted-foreground">Competition</div>
              </div>
            </div>
          </div>
          
          <div className="relative animate-float">
            <Card className="overflow-hidden border-2 border-primary/20 bg-card/50 backdrop-blur-sm">
              <img 
                src={heroImage} 
                alt="Flyingdarts gaming platform" 
                className="w-full h-auto object-cover"
              />
            </Card>
            
            {/* Floating UI elements */}
            <div className="absolute -top-4 -right-4 animate-pulse-glow">
              <Card className="p-4 bg-primary text-primary-foreground">
                <div className="text-sm font-medium">Live Match</div>
                <div className="text-xs opacity-80">501 • Best of 3</div>
              </Card>
            </div>
            
            <div className="absolute -bottom-4 -left-4 animate-float" style={{ animationDelay: '1s' }}>
              <Card className="p-4 bg-accent text-accent-foreground">
                <div className="text-sm font-medium">+125 XP</div>
                <div className="text-xs opacity-80">Great shot!</div>
              </Card>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default HeroSection;