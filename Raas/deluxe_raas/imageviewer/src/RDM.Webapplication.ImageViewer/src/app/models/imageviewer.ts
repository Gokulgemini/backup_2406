import { BackImage } from './backImage';
import { FrontImage } from './frontImage';

export class Imageviewer {

    readonly FrontImage: FrontImage;
    readonly BackImage: BackImage;

    constructor(FrontImage: FrontImage, BackImage: BackImage) {
        this.FrontImage = FrontImage;
        this.BackImage = BackImage;
    }
}
