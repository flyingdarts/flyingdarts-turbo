import loginClient from "@/authressClient";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const Protected = () => {
  const navigate = useNavigate();
  const [ready, setReady] = useState(false);

  useEffect(() => {
    let mounted = true;
    (async () => {
      const hasSession = await loginClient.userSessionExists();
      if (!hasSession) {
        navigate("/");
        return;
      }
      if (mounted) setReady(true);
    })();
    return () => {
      mounted = false;
    };
  }, [navigate]);

  if (!ready) return null;
  return (
    <div className="container mx-auto px-6 py-24">
      <h1 className="text-3xl font-bold">Protected</h1>
      <p className="text-muted-foreground mt-2">Only visible when logged in.</p>
    </div>
  );
};

export default Protected;


