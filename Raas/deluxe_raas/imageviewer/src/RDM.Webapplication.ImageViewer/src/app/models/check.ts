import { Imageviewer } from './imageviewer';
import { BackImage } from './backImage';
import { FrontImage } from './frontImage';

export class Check extends Imageviewer {
    constructor(FrontImage: FrontImage, BackImage: BackImage) {
        super(FrontImage, BackImage);
    }
}
