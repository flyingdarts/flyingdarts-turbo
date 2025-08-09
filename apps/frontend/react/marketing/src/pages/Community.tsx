import Footer from "@/components/Footer";
import Navigation from "@/components/Navigation";

const Community = () => {
  return (
    <div className="min-h-screen bg-background">
      <Navigation />
      <main className="container mx-auto px-6 pt-28 pb-16">
        <div className="max-w-3xl mx-auto text-center space-y-6">
          <h1 className="text-4xl font-bold">Community</h1>
          <p className="text-muted-foreground">
            Connect with other players, share tips, and stay up to date with the latest
            Flyingdarts news.
          </p>
          <div>
            <a
              href="https://discord.gg/SyFzsEbfsk"
              className="inline-flex items-center justify-center rounded-md bg-primary px-6 py-3 text-primary-foreground hover:opacity-90 transition-opacity"
            >
              Join us on Discord
            </a>
          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
};

export default Community;


