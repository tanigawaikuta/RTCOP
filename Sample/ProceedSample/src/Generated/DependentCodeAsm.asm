public	?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z
public	?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z
public	?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVLayer1@45@@Z
public	?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVLayer2@45@@Z
extrn ??_7A@baselayer@@6B@:PTR
extrn ??_7?$LayerdObject@VA@baselayer@@@Core@RTCOP@@6B@:PTR
extrn	??_7A@Layer1@@6B@:PTR
extrn ??_7B@baselayer@@6B@:PTR
extrn ??_7?$LayerdObject@VB@baselayer@@@Core@RTCOP@@6B@:PTR
extrn	??_7B@Layer2@@6B@:PTR

_TEXT segment USE32
	align 16
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z proc
	mov eax, offset ??_7?$LayerdObject@VA@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z endp
?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z proc
	mov eax, offset ??_7?$LayerdObject@VB@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z endp

	align 16
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVLayer1@45@@Z proc
	mov eax, offset ??_7A@Layer1@@6B@
	ret
?GetVirtualFunctionTable@A@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVLayer1@45@@Z endp

	align 16
?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVLayer2@45@@Z proc
	mov eax, offset ??_7B@Layer2@@6B@
	ret
?GetVirtualFunctionTable@B@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVLayer2@45@@Z endp

_TEXT ends
	end
