public ?GetVirtualFunctionTable@Hello@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@34@@Z
public ?GetVirtualFunctionTable@Hello@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVJapanese@34@@Z
public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVHello@Japanese@@@Z
extrn ??_7?$LayerdObject@VHello@@@Core@RTCOP@@6B@:PTR
extrn ??_7Hello@Japanese@@6B@:PTR
extrn ?_RTCOP_FinalizePartialClass@Hello@Japanese@@AEAAXXZ:PROC

_TEXT segment
	align 16
?GetVirtualFunctionTable@Hello@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@34@@Z proc
	mov rax, offset ??_7?$LayerdObject@VHello@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@Hello@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@34@@Z endp

	align 16
?GetVirtualFunctionTable@Hello@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVJapanese@34@@Z proc
	mov rax, offset ??_7Hello@Japanese@@6B@
	ret
?GetVirtualFunctionTable@Hello@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVJapanese@34@@Z endp

	align 16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVHello@Japanese@@@Z proc
	mov rax, offset ?_RTCOP_FinalizePartialClass@Hello@Japanese@@AEAAXXZ
	ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVHello@Japanese@@@Z endp

_TEXT ends
	end
