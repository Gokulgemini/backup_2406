import { BackImage } from './backImage';
import { GeneralDocumentPage } from "./generalDocumentPage";
import { StringUtils } from "../utils/string.utils";

export class GeneralDocument {

    private _documentName: string;
    private _pages: GeneralDocumentPage [];

    constructor(DocumentName: string, Pages: GeneralDocumentPage []){
        this._documentName = DocumentName;
        this._pages = Pages;

        this.convertBackImageToFrontImage();
    }

    get DocumentName(): string{
        return this._documentName;
    }

    get Pages(): GeneralDocumentPage[]{
        return this._pages;
    }

    private addPage(page: GeneralDocumentPage){
        this._pages.push(page);
    }

    private removeAllPages(){
        this._pages = [];
    }

    private convertBackImageToFrontImage()  {
        
        if(this._pages.filter((page) => { return !StringUtils.isNullOrEmpty(page.BackImage.Content) }).length == 0) return;

        const pages = this._pages;
        
        this.removeAllPages();

        const emptyBackImage = new BackImage("", 0, 0);
        
        pages.forEach((page) => {            
            this.addPage(new GeneralDocumentPage(this._pages.length + 1, page.FrontImage, emptyBackImage));
            this.addPage(new GeneralDocumentPage(this._pages.length + 1, page.BackImage, emptyBackImage));
        });
    }
}
