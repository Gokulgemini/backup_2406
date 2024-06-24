import { state, trigger, style } from '@angular/animations';

export let rotate = trigger('rotatedState', [
    state('default', style({ transform: 'rotate(0deg)' })),
    state('rotated1', style({ transform: 'rotate(90deg)' })),
    state('rotated2', style({ transform: 'rotate(180deg)' })),
    state('rotated3', style({ transform: 'rotate(270deg)' }))
]);
