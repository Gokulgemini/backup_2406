import { ImagePosition } from './imagePosition';
import { ImageFace } from '../constants/imageFace';
import { ImageType } from '../constants/imageType';

export class ImageConfig {

    private _ImageName: string;

    ImageSource: string;
    ImageFit: string;
    ImageFace: any;
    ImageWidth: number;
    ImageHeight: number;
    ImageTotalPage: number;
    ImagePage: number;
    ImageDimensionRatio: number;
    ImagePosition: ImagePosition;
    ImageType: string;
    ImageFrontContent: string;
    ImageBackContent: string;

    constructor() {
        this._ImageName = '';

        this.ImageWidth = window.innerWidth / 2;
        this.ImageHeight = 0;
        this.ImageFace = ImageFace.Front;
        this.ImageFit = '';
        this.ImageSource = '';
        this.ImageType = ImageType.Check;
        this.ImageTotalPage = 0;
        this.ImagePage = 1;
        this.ImagePosition = new ImagePosition();
        this.ImageFrontContent = '';
        this.ImageBackContent = '';
    }

    get ImageName(): string {
        return this._ImageName;
    }

    set ImageName(value: string) {

        if (value == null) {
            value = '';
        }

        this._ImageName = value.trim();
    }

    ImageNameIsValid(): boolean {
        return this._ImageName.length <= 50;
    }
}
