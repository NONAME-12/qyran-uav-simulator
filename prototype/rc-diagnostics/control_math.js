'use strict';
(function(root,factory){const lib=factory();if(typeof module!=='undefined'&&module.exports)module.exports=lib;if(root)root.QyranInputMath=lib;})(typeof window!=='undefined'?window:null,function(){
const clamp=(v,a,b)=>Math.min(b,Math.max(a,v));
function curve(value,deadzone=0,expo=0){const d=clamp(deadzone,0,.9),e=clamp(expo,0,1),abs=Math.abs(value);if(abs<=d)return 0;const v=clamp((abs-d)/(1-d),0,1);return Math.sign(value)*((1-e)*v+e*v*v*v);}
function normalize(raw,{min=-1,center=0,max=1,reverse=false,kind='axis',deadzone=0,expo=0}={}){if(!Number.isFinite(raw)||!(min<max))throw Error('Invalid input/calibration');if(kind==='throttle'){let t=clamp((raw-min)/(max-min),0,1);return reverse?1-t:t;}if(!(min<center&&center<max))throw Error('Invalid center');let v=raw>=center?(raw-center)/(max-center):(raw-center)/(center-min);return curve(clamp(reverse?-v:v,-1,1),deadzone,expo);}
return {clamp,curve,normalize};});
