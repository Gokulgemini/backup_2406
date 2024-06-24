import { Imageviewer } from './imageviewer';
import { FrontImage } from './frontImage';
import { BackImage } from './backImage';

export class Remittance extends Imageviewer {
    private _isVirtual: boolean;

    constructor(IsVirtual: boolean, FrontImage: FrontImage, BackImage: BackImage){
        super(FrontImage, BackImage);

        this._isVirtual = IsVirtual;
    }

    get IsVirtual(): boolean{
        return this._isVirtual;
    }
}
