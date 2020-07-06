//================================================================================
// RTCOPManager_Private.h
//
// 役割: RTCOPManagerのプライベートクラス。
//================================================================================

#ifndef __RTCOP_CORE_RTCOPMANAGER_PRIVATE__
#define __RTCOP_CORE_RTCOPMANAGER_PRIVATE__

namespace RTCOP {
namespace Core {

class Layer;

// RTCOP管理
class RTCOPManager_Private
{
public:
	// コンストラクタ
	RTCOPManager_Private(int numOfLayers, int numOfBaseClasses, int* numOfBaseMethods);
	// デストラクタ
	virtual ~RTCOPManager_Private();
public:
	// 登録されたレイヤの集合
	Layer** _RegisterdLayers;
	// レイヤードオブジェクトのための仮想関数テーブルの集合
	volatile void*** _VirtualFunctionTables;
public:
	// レイヤの個数
	int _NumOfLayers;
	// ベースクラスの個数
	int _NumOfBaseClasses;
	// ベースメソッドの個数
	int* _NumOfBaseMethods;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_RTCOPMANAGER_PRIVATE__
