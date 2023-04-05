import { Button, Card } from "semantic-ui-react";
import { useStore } from "../../../app/strores/store";
import LoadingComponent from "../../../app/layout/LoadingComponent";


export default function ActivityDetails() {
    const { activityStore } = useStore();
    const { selectedActivity: activity, openForm, cancelSelectedActyvity } = activityStore;

    if (!activity) {// for not getting error 
        return <LoadingComponent/>;
    }

    return (
        <Card fluid>
            <img src={`/assets/categoryImages/${activity.category}.jpg`} alt={`${activity.category}`} />
            <Card.Content>
                <Card.Header>{activity.title}</Card.Header>
                <Card.Meta>
                    <span>{activity.date}</span>
                </Card.Meta>
                <Card.Description>
                    {activity.description}
                </Card.Description>
            </Card.Content>
            <Card.Content extra>
                <Button.Group>
                    <Button onClick={() => openForm(activity.id)} basic color="blue" content='Edit' />
                    <Button onClick={cancelSelectedActyvity} basic color="grey" content='Cancel'/>
                </Button.Group>
            </Card.Content>
        </Card>
    )
}