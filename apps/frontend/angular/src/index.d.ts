export { };

declare global {
    interface Window {
        stream: any;
        FB: any;
        fbAsyncInit: () => void;
    }
}