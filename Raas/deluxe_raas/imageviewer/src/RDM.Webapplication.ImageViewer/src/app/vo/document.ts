import { ImageType } from '../constants/imageType';
import { Imageviewer } from '../models/imageviewer';

export class ImageDocument {
    Images: Imageviewer[];
    ItemType: string;
    DocumentName: string;

    constructor(){
        this.Images = [];
        this.DocumentName = "";
        this.ItemType = ImageType.Check;
    }
}