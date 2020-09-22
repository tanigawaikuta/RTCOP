public ?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z
public ?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z
public ?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z
public ?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer2@45@@Z
public ?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer2@45@@Z
public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVA@Layer1@@@Z
public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVA@Layer2@@@Z
public ?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVB@Layer2@@@Z
extrn ??_7?$LayerdObject@VA@baselayer@@@Core@RTCOP@@6B@:PTR
extrn ??_7A@Layer1@@6B@:PTR
extrn ??_7A@Layer2@@6B@:PTR
extrn ??_7?$LayerdObject@VB@baselayer@@@Core@RTCOP@@6B@:PTR
extrn ??_7B@Layer2@@6B@:PTR
extrn ?_RTCOP_FinalizePartialClass@A@Layer1@@AEAAXXZ:PROC
extrn ?_RTCOP_FinalizePartialClass@A@Layer2@@AEAAXXZ:PROC
extrn ?_RTCOP_FinalizePartialClass@B@Layer2@@AEAAXXZ:PROC

_TEXT segment
	align 16
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z proc
	mov rax, offset ??_7?$LayerdObject@VA@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z endp
	align 16
?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z proc
	mov rax, offset ??_7?$LayerdObject@VB@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@45@@Z endp

	align 16
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z proc
	mov rax, offset ??_7A@Layer1@@6B@
	ret
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer1@45@@Z endp

	align 16
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer2@45@@Z proc
	mov rax, offset ??_7A@Layer2@@6B@
	ret
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer2@45@@Z endp

	align 16
?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer2@45@@Z proc
	mov rax, offset ??_7B@Layer2@@6B@
	ret
?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVLayer2@45@@Z endp

	align 16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVA@Layer1@@@Z proc
	mov rax, offset ?_RTCOP_FinalizePartialClass@A@Layer1@@AEAAXXZ
	ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVA@Layer1@@@Z endp

	align 16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVA@Layer2@@@Z proc
	mov rax, offset ?_RTCOP_FinalizePartialClass@A@Layer2@@AEAAXXZ
	ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVA@Layer2@@@Z endp

	align 16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVB@Layer2@@@Z proc
	mov rax, offset ?_RTCOP_FinalizePartialClass@B@Layer2@@AEAAXXZ
	ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVB@Layer2@@@Z endp

_TEXT ends
	end
