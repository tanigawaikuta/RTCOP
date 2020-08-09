//================================================================================
// Layer.h
//
// 役割: レイヤごとの情報を扱うクラス。
//       パーシャルクラスのための仮想関数テーブルや、
//       Proceedを実現するための仮想関数テーブルを持っている。
//================================================================================

#ifndef __RTCOP_CORE_LAYER__
#define __RTCOP_CORE_LAYER__

namespace RTCOP {
namespace Core {

// レイヤの状態を表す列挙型
enum class LayerState : unsigned char
{
	Inactive = 0,		// 非アクティブ
	Active = 1,			// アクティブ
};

// レイヤごとの情報を扱う
class Layer
{
public:
	// コンストラクタ
	explicit Layer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	// デストラクタ
	virtual ~Layer();
public:
	// IDの取得
	const int GetID() const;
	// 名前の取得
	const char* const GetName() const;
	// パーシャルクラスのための仮想関数テーブルの取得
	volatile void** const GetVirtualFunctionTable(int classid) const;
	// Proceedを実現するための仮想関数テーブルの取得
	volatile void** const GetVirtualFunctionTableForProceeding(int classid) const;
	// レイヤ状態のゲッタ
	LayerState GetLayerState() const;
private:
	// レイヤ状態のセッタ
	void SetLayerState(LayerState state);
	// 前に連結されているアクティブなレイヤのゲッタ
	Layer* const GetLinkedPrevLayer() const;
	// 後に連結されているアクティブなレイヤのゲッタ
	Layer* const GetLinkedNextLayer() const;
	// 前に連結されているアクティブなレイヤのセッタ
	void SetLinkedPrevLayer(Layer* const layer);
	// 後に連結されているアクティブなレイヤのセッタ
	void SetLinkedNextLayer(Layer* const layer);
protected:
	// アクティベート開始時に実行される
	virtual void OnActivating();
	// アクティベート終了時に実行される
	virtual void OnActivated();
	// ディアクティベート開始時に実行される
	virtual void OnDeactivating();
	// ディアクティベート終了時に実行される
	virtual void OnDeactivated();
protected:
	// レイヤードオブジェクトの初期化
	virtual void* InitializeLayerdObject(void* obj, int classID) = 0;
public:
	// フレンドクラス
	friend class LayerActivater;
	friend class LayerdObjectInitializer;
protected:
	// プライベートクラス
	class Layer_Private* _Private;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_LAYER__
