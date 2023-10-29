
export interface WebSocketRequest {
}
export interface MessageRequest extends WebSocketRequest {
    Date: Date;
    Message: string;
    Owner: string;
}