const GeometricShapes = () => {
  return (
    <div className="absolute inset-0 overflow-hidden pointer-events-none">
      {/* Large pink triangle */}
      <div 
        className="absolute top-0 right-0 w-[800px] h-[600px] bg-accent opacity-80 animate-float"
        style={{
          clipPath: 'polygon(50% 0%, 100% 100%, 0% 100%)',
          transform: 'rotate(45deg) translate(200px, -100px)'
        }}
      />
      
      {/* Medium purple shape */}
      <div 
        className="absolute bottom-0 left-0 w-[600px] h-[400px] bg-primary opacity-60 animate-float"
        style={{
          clipPath: 'polygon(0% 0%, 70% 0%, 100% 100%, 0% 80%)',
          animationDelay: '2s'
        }}
      />
      
      {/* Small accent triangles */}
      <div 
        className="absolute top-1/3 left-1/4 w-32 h-32 bg-accent opacity-40 animate-pulse-glow"
        style={{
          clipPath: 'polygon(50% 0%, 0% 100%, 100% 100%)',
          animationDelay: '1s'
        }}
      />
      
      <div 
        className="absolute bottom-1/4 right-1/3 w-24 h-24 bg-primary opacity-50 animate-float"
        style={{
          clipPath: 'polygon(50% 0%, 0% 100%, 100% 100%)',
          animationDelay: '3s'
        }}
      />
    </div>
  );
};

export default GeometricShapes;