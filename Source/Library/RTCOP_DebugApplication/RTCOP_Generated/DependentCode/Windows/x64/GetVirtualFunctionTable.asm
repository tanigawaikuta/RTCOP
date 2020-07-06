;
;

public	?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@34@@Z ; RTCOP::Generated::DependentCode::HelloClass::GetVirtualFunctionTable
public	?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVJapaneseLayer@34@@Z ; RTCOP::Generated::DependentCode::HelloClass::GetVirtualFunctionTable
public	?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVEnglishLayer@34@@Z ; RTCOP::Generated::DependentCode::HelloClass::GetVirtualFunctionTable
extrn	??_7?$LayerdObject@VHello@@@Core@RTCOP@@6B@:PTR
extrn	??_7JapaneseLayer_Hello@Generated@RTCOP@@6B@:PTR
extrn	??_7EnglishLayer_Hello@Generated@RTCOP@@6B@:PTR

_TEXT	segment

        align	16
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@34@@Z  proc
		mov		rax,	offset ??_7?$LayerdObject@VHello@@@Core@RTCOP@@6B@
		ret
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVBaseLayer@34@@Z  endp

        align	16
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVJapaneseLayer@34@@Z  proc
		mov		rax,	offset ??_7JapaneseLayer_Hello@Generated@RTCOP@@6B@
		ret
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVJapaneseLayer@34@@Z  endp

        align	16
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVEnglishLayer@34@@Z  proc
		mov		rax,	offset ??_7EnglishLayer_Hello@Generated@RTCOP@@6B@
		ret
?GetVirtualFunctionTable@HelloClass@DependentCode@Generated@RTCOP@@YAPEAPECXPEAVEnglishLayer@34@@Z  endp

_TEXT	ends
		end
