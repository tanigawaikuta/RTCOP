public ?GetVirtualFunctionTable@SuperA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z
public ?GetVirtualFunctionTable@SubA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z
public ?GetVirtualFunctionTable@SuperA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z
public ?GetVirtualFunctionTable@SubA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z
public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVSuperA@Layer1@@@Z
public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVSubA@Layer1@@@Z
extrn ??_7?$LayerdObject@VSuperA@baselayer@@@Core@RTCOP@@6B@:PTR
extrn ??_7SuperA@Layer1@@6B@:PTR
extrn ??_7?$LayerdObject@VSubA@baselayer@@@Core@RTCOP@@6B@:PTR
extrn ??_7SubA@Layer1@@6B@:PTR
extrn ?_RTCOP_FinalizePartialClass@SuperA@Layer1@@AEAAXXZ:PROC
extrn ?_RTCOP_FinalizePartialClass@SubA@Layer1@@AEAAXXZ:PROC

_TEXT segment
	align 16
?GetVirtualFunctionTable@SuperA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z proc
	mov rax, offset ??_7?$LayerdObject@VSuperA@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@SuperA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z endp
	align 16
?GetVirtualFunctionTable@SubA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z proc
	mov rax, offset ??_7?$LayerdObject@VSubA@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@SubA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z endp

	align 16
?GetVirtualFunctionTable@SuperA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z proc
	mov rax, offset ??_7SuperA@Layer1@@6B@
	ret
?GetVirtualFunctionTable@SuperA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z endp

	align 16
?GetVirtualFunctionTable@SubA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z proc
	mov rax, offset ??_7SubA@Layer1@@6B@
	ret
?GetVirtualFunctionTable@SubA@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z endp

	align 16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVSuperA@Layer1@@@Z proc
	mov rax, offset ?_RTCOP_FinalizePartialClass@SuperA@Layer1@@AEAAXXZ
	ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVSuperA@Layer1@@@Z endp

	align 16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVSubA@Layer1@@@Z proc
	mov rax, offset ?_RTCOP_FinalizePartialClass@SubA@Layer1@@AEAAXXZ
	ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVSubA@Layer1@@@Z endp

_TEXT ends
	end
