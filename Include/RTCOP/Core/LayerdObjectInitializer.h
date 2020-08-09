//================================================================================
// LayerdObjectInitializer.h
//
// 役割: 与えられたレイヤードなオブジェクトの初期化を行うクラス。
//       このクラスでは、レイヤードオブジェクトの仮想関数テーブルの書き換えなどを行う
//================================================================================

#ifndef __RTCOP_CORE_LAYERDOBJECTINITIALIZER__
#define __RTCOP_CORE_LAYERDOBJECTINITIALIZER__

namespace RTCOP {
namespace Core {

class RTCOPManager;

// 与えられたレイヤードなオブジェクトの初期化を行う
class LayerdObjectInitializer
{
public:
	// コンストラクタ
	explicit LayerdObjectInitializer(RTCOPManager* manager);
	// デストラクタ
	virtual ~LayerdObjectInitializer();
public:
	// レイヤードオブジェクトの初期化
	virtual void* InitializeLayerdObject(void* obj, int classid, int size);
protected:
	// プライベートクラス
	class LayerdObjectInitializer_Private* _Private;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_LAYERDOBJECTINITIALIZER__
