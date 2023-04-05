import { observer } from "mobx-react-lite";
import { Dimmer, Loader } from "semantic-ui-react";

interface Props {
    inverted?: boolean;
    content?: string;
}

export default observer(function LoadingComponent({ inverted = true, content = 'Loading...' }: Props) {

    return (
        <Dimmer active={true} inverted={inverted}>
            <Loader content={content} />
        </Dimmer>
    )
});