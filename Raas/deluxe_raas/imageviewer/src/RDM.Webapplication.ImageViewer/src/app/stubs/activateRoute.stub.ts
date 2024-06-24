import { ParamMap, convertToParamMap } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

export class ActivatedRouteStub {

  private subjectParamMap = new BehaviorSubject(convertToParamMap(this.testParamMap));
  paramMap = this.subjectParamMap.asObservable();

  private _testParamMap: ParamMap;
  get testParamMap() {
      return this._testParamMap;
  }
  set testParamMap(params: {}) {
      this._testParamMap = convertToParamMap(params);
      this.subjectParamMap.next(this._testParamMap);
  }

  private subjectQueryParamMap = new BehaviorSubject(convertToParamMap(this.testParamMap));
  queryParamMap = this.subjectQueryParamMap.asObservable();

  private _testQueryParamMap: ParamMap;
  get testQueryParamMap() {
      return this._testQueryParamMap;
  }
  set testQueryParamMap(params: {}) {
      this._testQueryParamMap = convertToParamMap(params);
      this.subjectQueryParamMap.next(this._testQueryParamMap);
  }

  get snapshot() {
      return {
          paramMap: this.testParamMap,
          queryParamMap: this.testQueryParamMap
      };
  }

}
