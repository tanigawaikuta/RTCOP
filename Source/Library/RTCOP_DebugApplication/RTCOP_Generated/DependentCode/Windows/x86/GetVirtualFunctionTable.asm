;
;

public	?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@34@@Z ; RTCOP::Generated::DependentCode::HelloClass::GetVirtualFunctionTable
public	?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPAPCXPAVJapaneseLayer@34@@Z ; RTCOP::Generated::DependentCode::HelloClass::GetVirtualFunctionTable
public	?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPAPCXPAVEnglishLayer@34@@Z ; RTCOP::Generated::DependentCode::HelloClass::GetVirtualFunctionTable
extrn	??_7Hello@baselayer@@6B@:PTR
extrn	??_7?$LayerdObject@VHello@baselayer@@@Core@RTCOP@@6B@:PTR
extrn	??_7JapaneseLayer_Hello@Generated@RTCOP@@6B@:PTR
extrn	??_7EnglishLayer_Hello@Generated@RTCOP@@6B@:PTR

_TEXT	segment USE32

		align	16
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@34@@Z  proc
		mov		eax,	offset ??_7?$LayerdObject@VHello@baselayer@@@Core@RTCOP@@6B@
		ret
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPAPCXPAVBaseLayer@34@@Z  endp

		align	16
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPAPCXPAVJapaneseLayer@34@@Z  proc
		mov		eax,	offset ??_7JapaneseLayer_Hello@Generated@RTCOP@@6B@
		ret
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPAPCXPAVJapaneseLayer@34@@Z  endp

		align	16
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPAPCXPAVEnglishLayer@34@@Z  proc
		mov		eax,	offset ??_7EnglishLayer_Hello@Generated@RTCOP@@6B@
		ret
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPAPCXPAVEnglishLayer@34@@Z  endp

_TEXT	ends
		end
