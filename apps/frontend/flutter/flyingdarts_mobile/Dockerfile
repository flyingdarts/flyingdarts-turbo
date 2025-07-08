# Use an official Nginx image as the base image
FROM nginx:1.25.3-alpine-slim

# Set the working directory to /app
WORKDIR /app

# Copy the build files of your Flutter web app to the container
COPY build/web /app

# Copy the Nginx configuration file to the container
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port 80 to the outside world
EXPOSE 80

# Start Nginx when the container is run
CMD ["nginx", "-g", "daemon off;"]