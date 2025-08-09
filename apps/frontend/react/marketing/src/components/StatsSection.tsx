import { Card, CardContent } from "@/components/ui/card";

const StatsSection = () => {
  const stats = [
    {
      number: "10,000+",
      label: "Active Players",
      description: "Growing community worldwide"
    },
    {
      number: "1M+",
      label: "Games Played",
      description: "Matches completed daily"
    },
    {
      number: "50+",
      label: "Countries",
      description: "Players from around the globe"
    },
    {
      number: "99.9%",
      label: "Uptime",
      description: "Reliable gaming experience"
    }
  ];

  return (
    <section className="py-16 relative">
      <div className="container mx-auto px-6">
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-6">
          {stats.map((stat, index) => (
            <Card 
              key={index} 
              className="text-center bg-card/30 backdrop-blur-sm border-primary/20 hover:border-accent/50 transition-all duration-300"
            >
              <CardContent className="p-6">
                <div className="text-3xl lg:text-4xl font-bold bg-gradient-primary bg-clip-text text-transparent mb-2">
                  {stat.number}
                </div>
                <div className="text-lg font-semibold text-foreground mb-1">
                  {stat.label}
                </div>
                <div className="text-sm text-muted-foreground">
                  {stat.description}
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    </section>
  );
};

export default StatsSection;