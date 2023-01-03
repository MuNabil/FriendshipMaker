// To recieve the Group that will send from the API when message group update.
export interface Group {
    name: string;
    connections: Connection[];
}

export interface Connection {
    connectionId: string;
    username: string;
}