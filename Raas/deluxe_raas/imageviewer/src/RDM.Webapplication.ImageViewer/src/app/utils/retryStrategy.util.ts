import { Observable, timer } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

export const retryStrategy = ({
  maxRetryAttempts = 3,
  scalingDuration = 1000,
  includedStatusCodes = []
}: {
  maxRetryAttempts?: number,
  scalingDuration?: number,
  includedStatusCodes?: number[]
} = {}) => (attempts: Observable<any>) => {
  return attempts.pipe(
    mergeMap((error, i) => {

      const retryAttempt = i++;
      
      if (retryAttempt >= maxRetryAttempts || !includedStatusCodes.find(e => e === error.status)) {
        throw error;
      }
      
      return timer(retryAttempt * scalingDuration);
    })
  );
};
