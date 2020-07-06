//================================================================================
// RTCOPManager.h
//
// 役割: RTCOPアプリケーション全体を管理するためのクラス。
//       このクラスでは、レイヤ、仮想関数テーブルなどを管理し、
//       様々なRTCOP機能実現オブジェクトから参照される。
//================================================================================

#ifndef __RTCOP_CORE_RTCOPMANAGER__
#define __RTCOP_CORE_RTCOPMANAGER__

namespace RTCOP {
namespace Core {

class Layer;

// RTCOP管理
class RTCOPManager
{
public:
	// コンストラクタ
	RTCOPManager(int numOfLayers, int numOfBaseClasses, int* numOfBaseMethods);
	// デストラクタ
	virtual ~RTCOPManager();
public:
	// レイヤの登録
	void RegisterLayer(Layer* layer);
	// レイヤの取得
	Layer* const GetLayer(int layerid);
	// クラスIDで指定した仮想関数テーブルを取得
	volatile void** const GetVirtualFunctionTable(int classid);
	// レイヤの個数を取得
	const int GetNumOfLayers();
	// ベースクラスの個数を取得
	const int GetNumOfBaseClasses();
	// ベースメソッドの個数を取得
	int* const GetNumOfBaseMethods();
	// ベースメソッドの個数を取得
	const int GetNumOfBaseMethods(int classid);
protected:
	// プライベートクラス
	class RTCOPManager_Private* _Private;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_RTCOPMANAGER__
