import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Users, Trophy, Target, Gamepad2, TrendingUp, Shield } from "lucide-react";

const FeaturesSection = () => {
  const features = [
    {
      icon: <Users className="w-8 h-8" />,
      title: "Social Gaming",
      description: "Connect with friends, create private rooms, and build your darts community. Challenge players worldwide or keep it intimate with your crew."
    },
    {
      icon: <Trophy className="w-8 h-8" />,
      title: "Tournaments",
      description: "Compete in daily tournaments, seasonal championships, and special events. Climb the leaderboards and earn exclusive rewards."
    },
    {
      icon: <Target className="w-8 h-8" />,
      title: "Precision Scoring",
      description: "Advanced scoring system with multiple game modes including 501, Cricket, and custom variants. Real-time statistics and performance tracking."
    },
    {
      icon: <Gamepad2 className="w-8 h-8" />,
      title: "Multiple Game Modes",
      description: "From classic 501 to innovative new formats. Practice mode, ranked matches, and casual games for every skill level and preference."
    },
    {
      icon: <TrendingUp className="w-8 h-8" />,
      title: "Performance Analytics",
      description: "Detailed statistics, progress tracking, and personalized insights to help you improve your game and reach new skill levels."
    },
    {
      icon: <Shield className="w-8 h-8" />,
      title: "Fair Play",
      description: "Advanced anti-cheat systems and fair matchmaking ensure every game is competitive and enjoyable for all skill levels."
    }
  ];

  return (
    <section id="features" className="py-24 relative overflow-hidden">
      <div className="container mx-auto px-6">
        <div className="text-center mb-16">
          <h2 className="text-4xl lg:text-5xl font-bold mb-6">
            Why Choose 
            <span className="bg-gradient-primary bg-clip-text text-transparent"> Flyingdarts</span>
          </h2>
          <p className="text-xl text-muted-foreground max-w-3xl mx-auto">
            Experience the perfect blend of traditional darts and cutting-edge technology. 
            Every feature designed to enhance your gaming experience.
          </p>
        </div>
        
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
          {features.map((feature, index) => (
            <Card 
              key={index} 
              className="group hover:border-primary/50 transition-all duration-300 hover:shadow-[0_0_30px_hsl(var(--primary)/0.3)] bg-card/50 backdrop-blur-sm"
            >
              <CardHeader>
                <div className="text-primary group-hover:text-accent transition-colors duration-300 mb-4">
                  {feature.icon}
                </div>
                <CardTitle className="text-xl">{feature.title}</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-muted-foreground leading-relaxed">
                  {feature.description}
                </p>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    </section>
  );
};

export default FeaturesSection;