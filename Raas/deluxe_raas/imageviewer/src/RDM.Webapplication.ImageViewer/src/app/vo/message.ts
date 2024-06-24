export class Message {

    public Type: string;
    public Value: any;

    constructor(type: string, value: any) {
        this.Type = type;
        this.Value = value;
    }
}
