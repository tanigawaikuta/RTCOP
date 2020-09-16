public ?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z
public ?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVEnglish@45@@Z
public ?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVJapanese@45@@Z
public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVHello@English@@@Z
public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVHello@Japanese@@@Z
extrn ??_7?$LayerdObject@VHello@baselayer@@@Core@RTCOP@@6B@:PTR
extrn ??_7Hello@English@@6B@:PTR
extrn ??_7Hello@Japanese@@6B@:PTR
extrn ?_RTCOP_FinalizePartialClass@Hello@English@@AEAAXXZ:PROC
extrn ?_RTCOP_FinalizePartialClass@Hello@Japanese@@AEAAXXZ:PROC

_TEXT segment
	align 16
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z proc
	mov rax, offset ??_7?$LayerdObject@VHello@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z endp

	align 16
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVEnglish@45@@Z proc
	mov rax, offset ??_7Hello@English@@6B@
	ret
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVEnglish@45@@Z endp

	align 16
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVJapanese@45@@Z proc
	mov rax, offset ??_7Hello@Japanese@@6B@
	ret
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVJapanese@45@@Z endp

	align 16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVHello@English@@@Z proc
	mov rax, offset ?_RTCOP_FinalizePartialClass@Hello@English@@AEAAXXZ
	ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVHello@English@@@Z endp

	align 16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVHello@Japanese@@@Z proc
	mov rax, offset ?_RTCOP_FinalizePartialClass@Hello@Japanese@@AEAAXXZ
	ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVHello@Japanese@@@Z endp

_TEXT ends
	end
