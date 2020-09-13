public	?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z
public	?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVEnglish@45@@Z
public	?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVJapanese@45@@Z
extrn ??_7Hello@baselayer@@6B@:PTR
extrn ??_7?$LayerdObject@VHello@baselayer@@@Core@RTCOP@@6B@:PTR
extrn	??_7Hello@English@@6B@:PTR
extrn	??_7Hello@Japanese@@6B@:PTR

_TEXT segment USE32
	align 16
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z proc
	mov eax, offset ??_7?$LayerdObject@VHello@baselayer@@@Core@RTCOP@@6B@
	ret
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@45@@Z endp

	align 16
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVEnglish@45@@Z proc
	mov eax, offset ??_7Hello@English@@6B@
	ret
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVEnglish@45@@Z endp

	align 16
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVJapanese@45@@Z proc
	mov eax, offset ??_7Hello@Japanese@@6B@
	ret
?GetVirtualFunctionTable@Hello@baselayer@DependentCode@Generated@RTCOP@@YAPAPCXPAVJapanese@45@@Z endp

_TEXT ends
	end
