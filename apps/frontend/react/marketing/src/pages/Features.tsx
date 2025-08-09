import Footer from "@/components/Footer";
import Navigation from "@/components/Navigation";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Clock, Eye, Gamepad2, Mic, Target, Trophy, Users } from "lucide-react";

const Features = () => {
  const features = [
    {
      icon: <Target className="w-8 h-8" />,
      title: "X01 public games",
      description:
        "Play classic X01 (301/501/701) in public games with fair play and live scoring.",
    },
    {
      icon: <Users className="w-8 h-8" />,
      title: "Friends",
      description:
        "Add friends, see who's online, and invite them to play. Create private rooms or team up for public matches—stay connected and keep the rivalry going.",
    },
    {
      icon: <Mic className="w-8 h-8" />,
      title: "iOS scoring app",
      description:
        "Input scores hands‑free with speech‑to‑text on your iPhone or iPad. Fast, accurate voice scoring keeps you focused on the board.",
    },
    {
      icon: <Gamepad2 className="w-8 h-8" />,
      title: "Other game modes",
      description: "More ways to play are on the way.",
      comingSoon: true,
    },
    {
      icon: <Clock className="w-8 h-8" />,
      title: "Queues",
      description: "Smart matchmaking queues for faster games.",
      comingSoon: true,
    },
    {
      icon: <Trophy className="w-8 h-8" />,
      title: "Tournaments",
      description: "Compete in brackets, seasons, and events.",
      comingSoon: true,
    },
    {
      icon: <Eye className="w-8 h-8" />,
      title: "Spectate mode",
      description: "Watch live games and follow top players.",
      comingSoon: true,
    },
  ];

  return (
    <div className="min-h-screen bg-background">
      <Navigation />
      <main className="container mx-auto px-6 pt-28 pb-20">
        <div className="text-center max-w-2xl mx-auto mb-12">
          <h1 className="text-4xl lg:text-5xl font-bold mb-4">Features</h1>
          <p className="text-lg text-muted-foreground">
            What you can play and how you can connect—built for fun, competition, and community.
          </p>
        </div>

        <div className="grid gap-8 md:grid-cols-2 lg:grid-cols-3">
          {features.map((feature: { icon: JSX.Element; title: string; description: string; comingSoon?: boolean }, index) => (
            <Card
              key={index}
              className={`group hover:border-primary/50 transition-all duration-300 hover:shadow-[0_0_30px_hsl(var(--primary)/0.25)] bg-card/50 backdrop-blur-sm ${
                feature.comingSoon ? "opacity-50 grayscale" : ""
              }`}
            >
              <CardHeader>
                <div className="text-primary group-hover:text-accent transition-colors duration-300 mb-4">
                  {feature.icon}
                </div>
                <CardTitle className="text-xl">{feature.title}</CardTitle>
                {feature.comingSoon && (
                  <Badge variant="secondary" className="mt-2 w-fit">
                    Coming soon
                  </Badge>
                )}
              </CardHeader>
              <CardContent>
                <p className="text-muted-foreground leading-relaxed">{feature.description}</p>
              </CardContent>
            </Card>
          ))}
        </div>
      </main>
      <Footer />
    </div>
  );
};

export default Features;


