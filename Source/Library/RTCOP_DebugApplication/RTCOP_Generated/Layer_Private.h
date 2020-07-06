//================================================================================
// Layer_Private.h
//
// 役割: Layerのプライベートクラス。
//================================================================================

#ifndef __RTCOP_CORE_LAYER_PRIVATE__
#define __RTCOP_CORE_LAYER_PRIVATE__

namespace RTCOP {
namespace Core {

class Layer;

// レイヤの状態を表す列挙型
enum class LayerState : unsigned char;

// レイヤごとの情報を扱う
class Layer_Private
{
public:
	// コンストラクタ・デストラクタ
	Layer_Private(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	~Layer_Private();
public:
	// ID
	const int _ID;
	// レイヤ名
	const char* const _Name;
	// ベースクラスの個数（デストラクタ用）
	int _NumOfBaseClasses;
	// パーシャルクラスのための仮想関数テーブル
	volatile void*** _VirtualFunctionTables;
	// Proceedを実現するための仮想関数テーブル
	volatile void*** _VirtualFunctionTablesForProceeding;
	// 前後に連結されているアクティブなレイヤ
	Layer* _LinkedPrevLayer;
	Layer* _LinkedNextLayer;
	// レイヤの状態
	LayerState _LayerState;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_LAYER_PRIVATE__
