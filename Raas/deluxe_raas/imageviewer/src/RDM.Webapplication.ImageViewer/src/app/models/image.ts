export abstract class Image {
    private _content = '';
    private _width = 0;
    private _height = 0;

    constructor(Content: string, Width: number, Height: number) {
        this._content = Content;
        this._width = Width;
        this._height = Height;
    }

    get Content(): string {
        return this._content;
    }

    get Width(): number {
        return this._width;
    }

    get Height(): number {
        return this._height;
    }
}
