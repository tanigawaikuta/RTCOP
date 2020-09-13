//================================================================================
// JapaneseLayer.h
//
// 役割: JapaneseLayerを表すクラス。自動生成。
//================================================================================

#ifndef __RTCOP_GENERATED_JAPANESELAYER__
#define __RTCOP_GENERATED_JAPANESELAYER__

// 機能実現のためのヘッダ
#include "RTCOP/Core/Layer.h"
#include "RTCOP/Core/LayerdObject.h"
#include "RTCOP/Core/PartialClassMembers.h"
// JapaneseLayerに含まれるクラスのヘッダ
#include "Hello.h"

namespace RTCOP {
namespace Generated {

// Japaneseレイヤ
class JapaneseLayer : public Core::Layer
{
public:
	// インスタンスの取得
	static JapaneseLayer* GetInstance();
public:
	// コンストラクタ・デストラクタ
	explicit JapaneseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods);
	virtual ~JapaneseLayer() override;
protected:
	// レイヤードオブジェクトの初期化
	virtual void* InitializeLayerdObject(void* obj, int classID) override;
protected:
	// イベント発生時に実行されるメソッド
	virtual void _RTCOP_OnActivating() override;		// アクティベート開始時に実行される
	virtual void _RTCOP_OnActivated() override;			// アクティベート終了時に実行される
	virtual void _RTCOP_OnDeactivating() override;		// ディアクティベート開始時に実行される
	virtual void _RTCOP_OnDeactivated() override;		// ディアクティベート終了時に実行される
};

// JapaneseレイヤのHello
class JapaneseLayer_Hello : public Core::LayerdObject<baselayer::Hello>
{
public:
	// パーシャルクラスのメンバ変数
	class PartialClassMembers : public RTCOP::Core::PartialClassMembers
	{
	public:
		int _JapaneseMember;
	};
public:
	JapaneseLayer_Hello(int a);
	virtual void Print() override;
	virtual void Print2(char arg) override;
private:
	void _RTCOP_InitializePartialClass();
	void _RTCOP_FinalizePartialClass();
public:
	friend JapaneseLayer;
};

} // namespace Generated {}
} // namespace RTCOP {}

#endif //!__RTCOP_GENERATED_JAPANESELAYER__
