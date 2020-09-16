public	?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z
public	?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVLayer1@45@@Z
extrn ??_7Sample@baselayer@@6B@:PTR
extrn ??_7?$LayerdObject@VSample@baselayer@@@Core@RTCOP@@6B@:PTR
extrn	??_7Sample@Layer1@@6B@:PTR

_TEXT segment USE32
	align 16
?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z proc
	mov eax, offset ??_7?$LayerdObject@VSample@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z endp

	align 16
?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVLayer1@45@@Z proc
	mov eax, offset ??_7Sample@Layer1@@6B@
	ret
?GetVirtualFunctionTable@Sample@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVLayer1@45@@Z endp

_TEXT ends
	end
