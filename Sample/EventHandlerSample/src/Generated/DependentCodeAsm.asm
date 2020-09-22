public ?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z
public ?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z
public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVSample@Layer1@@@Z
extrn ??_7?$LayerdObject@VSample@baselayer@@@Core@RTCOP@@6B@:PTR
extrn ??_7Sample@Layer1@@6B@:PTR
extrn ?_RTCOP_FinalizePartialClass@Sample@Layer1@@AEAAXXZ:PROC

_TEXT segment
	align 16
?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z proc
	mov rax, offset ??_7?$LayerdObject@VSample@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z endp

	align 16
?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z proc
	mov rax, offset ??_7Sample@Layer1@@6B@
	ret
?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z endp

	align 16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVSample@Layer1@@@Z proc
	mov rax, offset ?_RTCOP_FinalizePartialClass@Sample@Layer1@@AEAAXXZ
	ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVSample@Layer1@@@Z endp

_TEXT ends
	end
