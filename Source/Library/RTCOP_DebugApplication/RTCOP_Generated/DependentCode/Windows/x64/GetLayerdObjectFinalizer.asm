public		?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVJapaneseLayer_Hello@23@@Z
public		?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVEnglishLayer_Hello@23@@Z
extrn		?_RTCOP_FinalizePartialClass@JapaneseLayer_Hello@Generated@RTCOP@@AEAAXXZ:PROC		; RTCOP::Generated::JapaneseLayer_Hello::_FinalizePartialClass
extrn		?_RTCOP_FinalizePartialClass@EnglishLayer_Hello@Generated@RTCOP@@AEAAXXZ:PROC			; RTCOP::Generated::EnglishLayer_Hello::_FinalizePartialClass

_TEXT	segment

		align		16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVJapaneseLayer_Hello@23@@Z  proc
		mov			rax,	offset	?_RTCOP_FinalizePartialClass@JapaneseLayer_Hello@Generated@RTCOP@@AEAAXXZ
		ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVJapaneseLayer_Hello@23@@Z  endp

		align		16
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVEnglishLayer_Hello@23@@Z  proc
		mov			rax,	offset	?_RTCOP_FinalizePartialClass@EnglishLayer_Hello@Generated@RTCOP@@AEAAXXZ
		ret
?GetLayerdObjectFinalizer@DependentCode@Generated@RTCOP@@YAPECXPEAVEnglishLayer_Hello@23@@Z  endp

_TEXT	ends
		end
