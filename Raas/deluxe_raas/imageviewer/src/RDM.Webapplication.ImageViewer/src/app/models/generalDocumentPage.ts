import { Imageviewer } from "./imageviewer";
import { BackImage } from "./backImage";
import { FrontImage } from "./frontImage";

export class GeneralDocumentPage extends Imageviewer {
    private _pageNumber: number;

    constructor(PageNumber: number, FrontImage: FrontImage, BackImage: BackImage){
        super(FrontImage, BackImage);
        
        this._pageNumber = PageNumber;
    }

    get PageNumber(): number {
        return this._pageNumber;
    }
}